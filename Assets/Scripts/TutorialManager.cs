using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

enum TutorialState {
    Start,
    IntroduceChef,
    GrabHoe,
    WaitForTilling,
    GrabSeeds,
    WaitForSeeding,
    WaitForHarvest,
    WaitForIngredientDelivery,
    WaitForCooking,
    GrabDish,
    DeliverDish,
    End,
}

public class TutorialManager : MonoBehaviour {
    [SerializeField] private GameObject tutorialTextPrefab = null;
    [SerializeField] private Hoe hoe = null;
    [SerializeField] private IngredientDropOff kitchenCounter = null;
    [SerializeField] private Pot pot = null;
    [SerializeField] private Counter counter = null;

    public static TutorialManager Instance { get; private set; }
    private TutorialState _state = TutorialState.Start;
    private const float TutorialTextHeight = 2.5f;
    private TutorialText _tutorialText = null;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void ShowTutorialText(Vector3 position, string text) {
        HideTutorialText();
        position.y = TutorialTextHeight;
        _tutorialText = GameObject.Instantiate(tutorialTextPrefab, position, Quaternion.identity)
            .GetComponentInChildren<TutorialText>();
        Debug.Assert(_tutorialText is not null);
        _tutorialText.Text = text;
    }

    private void HideTutorialText() {
        if (_tutorialText is not null) {
            GameObject.Destroy(_tutorialText.gameObject);
        }
    }

    public void OnMeepleSelected(Meeple meeple) {
        if (_state == TutorialState.IntroduceChef) {
            ShowTutorialText(hoe.transform.position, "Right-click the hoe to grab it.");
            _state = TutorialState.GrabHoe;
        }
    }

    public void OnTillingCompleted() {
        if (_state == TutorialState.WaitForTilling) {
            ShowTutorialText(SeedsManager.Instance.Seeds.First().transform.position,
                "Right-click on some seeds to grab them.");
            _state = TutorialState.GrabSeeds;
        }
    }

    public void OnPickedUpItem(Carryable item) {
        if (_state == TutorialState.GrabSeeds && item is Seeds) {
            var ground = GameObject.FindObjectOfType<TilledGround>();
            ShowTutorialText(ground.transform.position, "Right-click tilled soil to plant the seeds.");
            _state = TutorialState.WaitForSeeding;
        }
        if (_state == TutorialState.WaitForHarvest && item.GetComponent<Plant>()) {
            ShowTutorialText(kitchenCounter.transform.position,
                "Right-click on the counter to bring the harvested wheat into the kitchen.");
            _state = TutorialState.WaitForIngredientDelivery;
        }
        if (_state == TutorialState.GrabDish && item is Dish) {
            ShowTutorialText(counter.transform.position,
                "Right-click the counter to sell the dish to a customer.");
            _state = TutorialState.DeliverDish;
        }
    }

    private IEnumerator HideTutorialTextAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        HideTutorialText();
        _state = TutorialState.End;
    }

    public void OnDishDelivered() {
        if (_state == TutorialState.DeliverDish) {
            ShowTutorialText(counter.transform.position,
                "Great! Keep in mind that some dishes are cooked from multiple ingredients.");
            StartCoroutine(HideTutorialTextAfterDelay(6f));
        }
    }

    public void OnIngredientDelivered(IngredientDropOff dropOff) {
        if (_state == TutorialState.WaitForIngredientDelivery) {
            ShowTutorialText(pot.transform.position,
                "Right-click the pot to cook a meal from all delivered ingredients.");
            _state = TutorialState.WaitForCooking;
        }
    }

    public void OnCookingFinished(Pot pot) {
        if (_state == TutorialState.WaitForCooking) {
            var dish = GameObject.FindObjectOfType<Dish>();
            ShowTutorialText(dish.transform.position, "Right-click the cooked dish to grab it.");
            _state = TutorialState.GrabDish;
        }
    }

    public void OnPlacedSeeds(TilledGround ground) {
        if (_state == TutorialState.WaitForSeeding) {
            ShowTutorialText(ground.transform.position,
                "Wait for the plant to grow. After that, right-click to harvest.");
            _state = TutorialState.WaitForHarvest;
        }
    }

    private void Update() {
        switch (_state) {
            case TutorialState.Start: {
                if (GameManager.Instance.Meeples.Any()) {
                    var meeple = GameManager.Instance.Meeples.First();
                    ShowTutorialText(meeple.transform.position,
                        "This is your robot chef. Left-click to select.");
                    _state = TutorialState.IntroduceChef;
                }
                break;
            }
            case TutorialState.GrabHoe: {
                var meeple = GameManager.Instance.Meeples.First();
                if (meeple.CurrentItem() is not null && meeple.CurrentItem() is Hoe) {
                    ShowTutorialText(meeple.transform.position, "Right-click on soil to till it.");
                    _state = TutorialState.WaitForTilling;
                }
                break;
            }
        }
    }
}